import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../environments/environment';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private token: string | null = null;
  private router = inject(Router);
  private notificationService = inject(NotificationService);

  login(): void {
    const state = this.generateRandomString(16);
    sessionStorage.setItem('oauth_state', state);

    const clientId = environment.twitch_client_id;
    const redirectUri = window.location.origin + '/'; // Redirect to home, where handleAuthentication is called
    const responseType = 'token'; // Or 'code' if you have a backend to exchange it
    const scope = 'user:read:email'; // Add any other scopes you need

    const authUrl = `https://id.twitch.tv/oauth2/authorize?client_id=${clientId}&redirect_uri=${redirectUri}&response_type=${responseType}&scope=${scope}&state=${state}`;

    window.location.href = authUrl;
  }

  private generateRandomString(length: number): string {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
  }

  // Check for a token in the URL, store it, and navigate to the dashboard.
  // Returns true if an OAuth callback was processed.
  handleAuthentication(): boolean {
    const hash = window.location.hash;
    const urlParams = new URLSearchParams(hash.substring(1)); // Use hash for fragment
    const token = urlParams.get('access_token');
    const state = urlParams.get('state');
    const storedState = sessionStorage.getItem('oauth_state');
    const error = urlParams.get('error');

    // Only process if this looks like an OAuth callback (has token or error in hash)
    if (!token && !error) {
      return false; // Not an OAuth callback, nothing to do
    }

    if (error) {
      const errorDescription = urlParams.get('error_description') || 'Unknown error';
      this.notificationService.showError(`OAuth error: ${error} - ${errorDescription}`);
      this.router.navigate(['/login']);
      return true;
    }

    if (!state || state !== storedState) {
      this.notificationService.showError('Invalid state parameter. Possible CSRF attack or expired session.', 'Received:', state, 'Expected:', storedState);
      this.router.navigate(['/login']);
      return true;
    }

    if (token) {
      this.setToken(token);
      sessionStorage.removeItem('oauth_state'); // Clean up the state after successful auth
      // Clear the hash from the URL and navigate to dashboard
      window.history.replaceState(null, '', window.location.pathname);
      this.router.navigate(['/dashboard']);
      return true;
    }

    return false;
  }

  // Store the JWT in session storage.
  setToken(token: string): void {
    this.token = token;
    sessionStorage.setItem('auth_token', token);
  }

  // Retrieve the JWT from session storage.
  getToken(): string | null {
    if (!this.token) {
      this.token = sessionStorage.getItem('auth_token');
    }
    return this.token;
  }

  // Check if the user is authenticated.
  isAuthenticated(): boolean {
    return this.getToken() !== null;
  }

  // Log the user out by clearing the token and redirecting to the login page.
  logout(): void {
    this.token = null;
    sessionStorage.removeItem('auth_token');
    sessionStorage.removeItem('oauth_state');
    this.router.navigate(['/login']);
  }
}