import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less'],
  standalone: true,
  imports: [CommonModule]
})
export class LoginComponent {
  login() {
    // Redirect to the backend's login endpoint, which will then redirect to Twitch.
    window.location.href = `${environment.apiUrl}/api/auth/login`;
  }
}
