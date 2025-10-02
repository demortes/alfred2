import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less'],
  standalone: true
})
export class LoginComponent {
  login() {
    // Redirect to the backend's login endpoint, which will then redirect to Twitch.
    window.location.href = `${environment.apiUrl}/api/auth/login`;
  }
}
