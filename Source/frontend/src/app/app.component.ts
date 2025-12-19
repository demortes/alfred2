import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from './sidebar/sidebar.component';
import { AuthService } from './auth.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, SidebarComponent, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.less'
})
export class AppComponent implements OnInit {
  title = 'alfred';
  showSidebar = false;
  private authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit(): void {
    // Handle OAuth callback if present
    const wasOAuthCallback = this.authService.handleAuthentication();

    // Update sidebar visibility initially and on navigation
    this.updateSidebarVisibility();

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.updateSidebarVisibility();
    });
  }

  private updateSidebarVisibility(): void {
    const isLoginPage = this.router.url === '/login' || this.router.url.startsWith('/login?');
    this.showSidebar = this.authService.isAuthenticated() && !isLoginPage;
  }
}
