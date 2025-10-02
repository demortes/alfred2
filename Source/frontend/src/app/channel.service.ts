import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from './auth.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChannelService {
  private apiUrl = `${environment.apiUrl}/api/channel`;

  constructor(private http: HttpClient, private authService: AuthService) { }

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  joinChannel(): Observable<any> {
    return this.http.post(`${this.apiUrl}/join`, {}, { headers: this.getAuthHeaders() });
  }

  leaveChannel(): Observable<any> {
    return this.http.post(`${this.apiUrl}/leave`, {}, { headers: this.getAuthHeaders() });
  }
}