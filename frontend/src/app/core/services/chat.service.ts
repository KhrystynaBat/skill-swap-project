import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { Observable, Subject, firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ChatConversation, ChatMessage, SendChatMessageRequest } from '../../models/chat.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private apiUrl = environment.apiUrl;
  private hubConnection: HubConnection | null = null;
  private connectionToken: string | null = null;
  private incomingMessageSubject = new Subject<ChatMessage>();

  incomingMessages$ = this.incomingMessageSubject.asObservable();

  getConversations(): Observable<ChatConversation[]> {
    return this.http.get<ChatConversation[]>(`${this.apiUrl}/chat/conversations`);
  }

  getMessages(partnerId: number): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.apiUrl}/chat/${partnerId}/messages`);
  }

  async startConnection(): Promise<void> {
    const token = this.authService.getToken();

    if (!token) {
      await this.stopConnection();
      return;
    }

    if (this.hubConnection && this.connectionToken !== token) {
      await this.stopConnection();
    }

    if (
      this.hubConnection?.state === HubConnectionState.Connected &&
      this.connectionToken === token
    ) {
      return;
    }

    if (!this.hubConnection) {
      this.hubConnection = new HubConnectionBuilder()
        .withUrl(`${this.apiUrl}/hubs/chat`, {
          accessTokenFactory: () => this.authService.getToken() || '',
        })
        .withAutomaticReconnect()
        .build();

      this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
        this.incomingMessageSubject.next(message);
      });
    }

    if (this.hubConnection.state === HubConnectionState.Disconnected) {
      await this.hubConnection.start();
      this.connectionToken = token;
    }
  }

  async sendMessage(receiverId: number, text: string): Promise<ChatMessage> {
    const payload = text.trim();

    if (!payload) {
      throw new Error('Message text is required');
    }

    try {
      await this.startConnection();

      if (this.hubConnection?.state === HubConnectionState.Connected) {
        return await this.hubConnection.invoke<ChatMessage>('SendMessage', receiverId, payload);
      }
    } catch (error) {
      console.warn('SignalR send failed, falling back to REST.', error);
    }

    const body: SendChatMessageRequest = { text: payload };
    return await firstValueFrom(
      this.http.post<ChatMessage>(`${this.apiUrl}/chat/${receiverId}/messages`, body),
    );
  }

  async stopConnection(): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.stop();
      this.hubConnection = null;
      this.connectionToken = null;
    }
  }
}
