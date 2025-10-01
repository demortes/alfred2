import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { ChannelService } from '../channel.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.less']
})
export class DashboardComponent {
  constructor(
    private authService: AuthService,
    private channelService: ChannelService
  ) { }

  joinChannel() {
    this.channelService.joinChannel().subscribe({
      next: (response) => console.log('Successfully joined channel:', response),
      error: (error) => console.error('Error joining channel:', error)
    });
  }

  leaveChannel() {
    this.channelService.leaveChannel().subscribe({
      next: (response) => console.log('Successfully left channel:', response),
      error: (error) => console.error('Error leaving channel:', error)
    });
  }

  logout() {
    this.authService.logout();
  }
}