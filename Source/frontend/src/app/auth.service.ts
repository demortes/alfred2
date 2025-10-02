import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private token: string | null = null;

  constructor(private router: Router) { }

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
  handleAuthentication(): void {
    const urlParams = new URLSearchParams(window.location.hash.substring(1)); // Use hash for fragment
    const token = urlParams.get('access_token');
    const state = urlParams.get('state');
    const storedState = sessionStorage.getItem('oauth_state');
    const error = urlParams.get('error');

    if (error) {
      console.error('OAuth error:', error);
      this.router.navigate(['/login']);
      return;
    }

    if (!state || state !== storedState) {
      console.error('Invalid state. CSRF attack suspected.');
      this.router.navigate(['/login']);
      return;
    }

    if (token) {
      this.setToken(token);
      // Navigate to the dashboard, removing the token from the URL.
      this.router.navigate(['/dashboard']);
    }
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