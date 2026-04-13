// export interface SearchUser {
//   id: number;
//   name: string;
//   city: string | null;
//   avatarUrl?: string;
//   level: number;
// }

export interface SearchUser {
  id: number;
  name: string;
  city?: string;
  avatarUrl?: string;

  teachSkills: {
    name: string;
    category: string;
    level: number;
  }[];

  learnSkills: {
    name: string;
    category: string;
    priority: number;
  }[];
}
