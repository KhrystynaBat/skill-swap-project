export interface ChatConversation {
  partnerId: number;
  partnerName: string;
  partnerAvatarUrl?: string | null;
  lastMessage: string;
  lastMessageAt: string;
}

export interface ChatMessage {
  id: number;
  senderId: number;
  receiverId: number;
  text: string;
  timestamp: string;
  isMine: boolean;
}

export interface SendChatMessageRequest {
  text: string;
}
