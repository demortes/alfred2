import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less'],
  standalone: true,
  imports: [CommonModule]
})
export class LoginComponent {

  constructor(private authService: AuthService) { }

  login() {
    this.authService.login();
  }
}
