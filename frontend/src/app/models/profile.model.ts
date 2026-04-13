export interface ProfileUser {
  id: number;
  name: string;
  email: string;
  avatarUrl: string | null;
  bio: string | null;
  city: string | null;
  role: string;
  createdAt: string;
}

export interface ProfileRating {
  average: number;
  count: number;
}

export interface ProfileResponse {
  user: ProfileUser;
  rating: ProfileRating;
}
