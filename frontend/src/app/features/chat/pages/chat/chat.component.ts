import { CommonModule } from '@angular/common';
import { AfterViewChecked, Component, ElementRef, OnDestroy, OnInit, ViewChild, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../../../../core/services/auth.service';
import { ChatService } from '../../../../core/services/chat.service';
import { UsersService } from '../../../../core/services/users.service';
import { ChatConversation, ChatMessage } from '../../../../models/chat.model';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss',
})
export class ChatComponent implements OnInit, AfterViewChecked, OnDestroy {
  @ViewChild('messagesList') private messagesList?: ElementRef<HTMLDivElement>;

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private authService = inject(AuthService);
  private chatService = inject(ChatService);
  private usersService = inject(UsersService);
  private subscription = new Subscription();
  private shouldScrollToBottom = false;

  conversations: ChatConversation[] = [];
  messages: ChatMessage[] = [];
  selectedPartnerId: number | null = null;
  selectedPartnerName = '';
  selectedPartnerAvatarUrl: string | null = null;
  draft = '';
  isLoadingConversations = true;
  isLoadingMessages = false;
  isSending = false;
  errorMessage = '';
  currentUserId: number | null = null;

  ngOnInit(): void {
    this.currentUserId = this.authService.getUserId();
    this.refreshCurrentUser();

    this.chatService.startConnection().catch(() => {
      this.errorMessage = 'Realtime connection is unavailable. Messages still work after refresh.';
    });

    this.subscription.add(
      this.chatService.incomingMessages$.subscribe((message) => {
        this.handleIncomingMessage(message);
      }),
    );

    this.subscription.add(
      this.route.paramMap.subscribe((params) => {
        const partnerId = Number(params.get('partnerId'));
        this.selectedPartnerId = Number.isFinite(partnerId) && partnerId > 0 ? partnerId : null;
        this.loadConversations();

        if (this.selectedPartnerId) {
          this.loadMessages(this.selectedPartnerId);
          this.loadPartner(this.selectedPartnerId);
        } else {
          this.messages = [];
          this.selectedPartnerName = '';
          this.selectedPartnerAvatarUrl = null;
        }
      }),
    );
  }

  ngAfterViewChecked(): void {
    if (!this.shouldScrollToBottom || !this.messagesList) {
      return;
    }

    const element = this.messagesList.nativeElement;
    element.scrollTop = element.scrollHeight;
    this.shouldScrollToBottom = false;
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
    this.chatService.stopConnection().catch((error) => {
      console.warn('Failed to stop chat connection.', error);
    });
  }

  selectConversation(conversation: ChatConversation): void {
    this.router.navigate(['/chat', conversation.partnerId]);
  }

  sendMessage(): void {
    if (!this.selectedPartnerId || !this.draft.trim() || this.isSending) {
      return;
    }

    const text = this.draft;
    this.draft = '';
    this.isSending = true;

    this.chatService
      .sendMessage(this.selectedPartnerId, text)
      .then((message) => {
        const sentMessage: ChatMessage = {
          ...this.normalizeMessage(message),
        };

        this.addMessageIfNew(sentMessage);
        this.upsertConversationFromMessage(sentMessage, this.selectedPartnerId!);
      })
      .catch((error) => {
        console.error('Send message error:', error);
        this.errorMessage = 'Failed to send message.';
        this.draft = text;
      })
      .finally(() => {
        this.isSending = false;
      });
  }

  trackConversation(_: number, conversation: ChatConversation): number {
    return conversation.partnerId;
  }

  trackMessage(_: number, message: ChatMessage): number {
    return message.id;
  }

  formatTime(value: string): string {
    return new Date(value).toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  private loadConversations(): void {
    this.isLoadingConversations = true;

    this.chatService.getConversations().subscribe({
      next: (conversations) => {
        this.conversations = conversations;
        this.errorMessage = '';
        this.isLoadingConversations = false;
        this.syncSelectedPartnerFromConversations();
        this.ensureSelectedConversationExists();
      },
      error: (error) => {
        console.error('Load conversations error:', error);
        this.errorMessage = 'Failed to load conversations.';
        this.isLoadingConversations = false;
      },
    });
  }

  private loadMessages(partnerId: number): void {
    this.isLoadingMessages = true;

    this.chatService.getMessages(partnerId).subscribe({
      next: (messages) => {
        this.messages = messages.map((message) => this.normalizeMessage(message));
        this.errorMessage = '';
        this.isLoadingMessages = false;
        this.shouldScrollToBottom = true;
        this.ensureSelectedConversationExists();
      },
      error: (error) => {
        console.error('Load messages error:', error);
        this.errorMessage = 'Failed to load messages.';
        this.isLoadingMessages = false;
      },
    });
  }

  private loadPartner(partnerId: number): void {
    this.usersService.getUserById(partnerId).subscribe({
      next: (user) => {
        this.selectedPartnerName = user.name;
        this.selectedPartnerAvatarUrl = user.avatarUrl || null;
        this.conversations = this.conversations.map((conversation) =>
          conversation.partnerId === partnerId
            ? {
                ...conversation,
                partnerName: user.name,
                partnerAvatarUrl: user.avatarUrl || null,
              }
            : conversation,
        );
        this.ensureSelectedConversationExists();
      },
    });
  }

  private handleIncomingMessage(message: ChatMessage): void {
    const normalizedMessage = this.normalizeMessage(message);

    this.upsertConversationFromMessage(normalizedMessage);

    if (this.selectedPartnerId && this.belongsToSelectedConversation(normalizedMessage)) {
      this.addMessageIfNew(normalizedMessage);
    }
  }

  private addMessageIfNew(message: ChatMessage): void {
    const normalizedMessage = this.normalizeMessage(message);

    if (this.messages.some((item) => item.id === message.id)) {
      return;
    }

    this.messages = [...this.messages, normalizedMessage].sort(
      (a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime(),
    );
    this.shouldScrollToBottom = true;
  }

  private belongsToSelectedConversation(message: ChatMessage): boolean {
    return message.senderId === this.selectedPartnerId || message.receiverId === this.selectedPartnerId;
  }

  private upsertConversationFromMessage(message: ChatMessage, partnerIdOverride?: number): void {
    const partnerId = partnerIdOverride || this.getPartnerId(message);

    if (!partnerId) {
      return;
    }

    const existing = this.conversations.find((item) => item.partnerId === partnerId);
    const updated: ChatConversation = {
      partnerId,
      partnerName: existing?.partnerName || this.selectedPartnerName || 'User',
      partnerAvatarUrl: existing?.partnerAvatarUrl || this.selectedPartnerAvatarUrl,
      lastMessage: message.text,
      lastMessageAt: message.timestamp,
    };

    this.conversations = [
      updated,
      ...this.conversations.filter((item) => item.partnerId !== partnerId),
    ].sort((a, b) => new Date(b.lastMessageAt).getTime() - new Date(a.lastMessageAt).getTime());
  }

  private syncSelectedPartnerFromConversations(): void {
    if (!this.selectedPartnerId) {
      return;
    }

    const conversation = this.conversations.find(
      (item) => item.partnerId === this.selectedPartnerId,
    );

    if (!conversation) {
      return;
    }

    this.selectedPartnerName = conversation.partnerName;
    this.selectedPartnerAvatarUrl = conversation.partnerAvatarUrl || null;
  }

  private ensureSelectedConversationExists(): void {
    if (!this.selectedPartnerId || !this.selectedPartnerName) {
      return;
    }

    const exists = this.conversations.some(
      (conversation) => conversation.partnerId === this.selectedPartnerId,
    );

    if (exists || this.messages.length === 0) {
      return;
    }

    const lastMessage = [...this.messages].sort(
      (a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime(),
    )[0];

    this.conversations = [
      {
        partnerId: this.selectedPartnerId,
        partnerName: this.selectedPartnerName,
        partnerAvatarUrl: this.selectedPartnerAvatarUrl,
        lastMessage: lastMessage.text,
        lastMessageAt: lastMessage.timestamp,
      },
      ...this.conversations,
    ];
  }

  private refreshCurrentUser(): void {
    this.authService.getMe().subscribe({
      next: (me) => {
        const userId = Number(me.userId);

        if (!Number.isFinite(userId) || userId <= 0) {
          return;
        }

        this.currentUserId = userId;
        this.messages = this.messages.map((message) => this.normalizeMessage(message));
      },
    });
  }

  private normalizeMessage(message: ChatMessage): ChatMessage {
    if (!this.currentUserId) {
      return message;
    }

    return {
      ...message,
      isMine: message.senderId === this.currentUserId,
    };
  }

  private getPartnerId(message: ChatMessage): number | null {
    if (!this.currentUserId) {
      return message.isMine ? message.receiverId : message.senderId;
    }

    if (message.senderId === this.currentUserId) {
      return message.receiverId;
    }

    if (message.receiverId === this.currentUserId) {
      return message.senderId;
    }

    return null;
  }
}
