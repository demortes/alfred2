import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth.service';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less'],
  standalone: true,
  imports: [RouterOutlet]
})
export class AppComponent implements OnInit {
  title = 'Alfred';

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.authService.handleAuthentication();
  }
}
