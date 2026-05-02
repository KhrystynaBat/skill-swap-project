import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/pages/login/login';
import { RegisterComponent } from './features/auth/pages/register/register';
import { ProfileComponent } from './features/profile/pages/profile/profile.component';
import { EditProfileComponent } from './features/profile/pages/edit-profile/edit-profile.component';
import { SearchUsersComponent } from './features/users/pages/search-users/search-users.component';
import { HomeComponent } from './features/home/pages/home/home.component';
import { MatchesComponent } from './features/matches/pages/matches/matches.component';
import { AboutComponent } from './features/home/pages/about/about.component';
import { ChatComponent } from './features/chat/pages/chat/chat.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'profile', component: ProfileComponent },
  { path: 'about', component: AboutComponent },
  { path: 'profile/edit', component: EditProfileComponent },
  { path: 'users/search', component: SearchUsersComponent },
  { path: 'chat', component: ChatComponent },
  { path: 'chat/:partnerId', component: ChatComponent },

  {
    path: 'users/:id',
    loadComponent: () =>
      import('./features/users/pages/user-profile/user-profile.component').then(
        (m) => m.UserProfileComponent,
      ),
  },
  { path: 'matches', component: MatchesComponent },

  { path: '**', redirectTo: '' },
];
