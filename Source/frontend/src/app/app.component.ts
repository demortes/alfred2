import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent implements OnInit {
  title = 'Alfred';

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.authService.handleAuthentication();
  }
}
