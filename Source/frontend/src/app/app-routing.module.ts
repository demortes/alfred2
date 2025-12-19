import { Routes } from '@angular/router';
import { MockWeatherComponent } from './mock-weather/mock-weather.component';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CommandsComponent } from './commands/commands.component';
import { LogsComponent } from './logs/logs.component';
import { RolesComponent } from './roles/roles.component';
import { SettingsComponent } from './settings/settings.component';
import { AuthGuard } from './auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'commands',
    component: CommandsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'logs',
    component: LogsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'roles',
    component: RolesComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'settings',
    component: SettingsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'weather',
    component: MockWeatherComponent,
    canActivate: [AuthGuard]
  },
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
