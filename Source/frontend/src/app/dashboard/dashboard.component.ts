import { Component, OnInit, signal, computed } from '@angular/core';
import { AuthService } from '../auth.service';
import { ChannelService } from '../channel.service';
import { BotSettingsService } from '../bot-settings.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BotSettings, ComponentSetting, TimeoutSetting } from '../models/bot-settings';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.less'],
  imports: [CommonModule, FormsModule]
})
export class DashboardComponent implements OnInit {
  settings = signal<BotSettings | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  // Computed signal for active components count
  activeComponentsCount = computed(() => {
    const components = this.settings()?.components;
    return components ? components.filter(c => c.enabled).length : 0;
  });

  constructor(
    private authService: AuthService,
    private channelService: ChannelService,
    private botSettingsService: BotSettingsService
  ) { }

  ngOnInit(): void {
    this.loadSettings();
  }

  loadSettings() {
    this.loading.set(true);
    this.error.set(null);

    this.botSettingsService.getSettings().subscribe({
      next: (settings) => {
        this.settings.set(settings);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading settings:', err);
        this.error.set('Failed to load bot settings');
        this.loading.set(false);
      }
    });
  }

  joinChannel() {
    this.loading.set(true);
    this.channelService.joinChannel().subscribe({
      next: (response) => {
        console.log('Successfully joined channel:', response);
        this.showSuccess('Successfully joined channel');
        this.loadSettings(); // Reload to get updated connection status
      },
      error: (error) => {
        console.error('Error joining channel:', error);
        this.error.set('Failed to join channel');
        this.loading.set(false);
      }
    });
  }

  leaveChannel() {
    this.loading.set(true);
    this.channelService.leaveChannel().subscribe({
      next: (response) => {
        console.log('Successfully left channel:', response);
        this.showSuccess('Successfully left channel');
        this.loadSettings(); // Reload to get updated connection status
      },
      error: (error) => {
        console.error('Error leaving channel:', error);
        this.error.set('Failed to leave channel');
        this.loading.set(false);
      }
    });
  }

  toggleComponent(component: ComponentSetting) {
    const newState = !component.enabled;
    this.botSettingsService.updateComponent(component.name, newState).subscribe({
      next: () => {
        component.enabled = newState;
        this.showSuccess(`${component.name} ${newState ? 'enabled' : 'disabled'}`);
      },
      error: (err) => {
        console.error(`Error updating ${component.name}:`, err);
        this.error.set(`Failed to update ${component.name}`);
      }
    });
  }

  updateTimeout(timeout: TimeoutSetting, event: Event) {
    const input = event.target as HTMLInputElement;
    const newValue = parseInt(input.value, 10);

    if (isNaN(newValue) || newValue < 0 || newValue > 300) {
      this.error.set('Timeout value must be between 0 and 300 seconds');
      input.value = timeout.valueSeconds.toString();
      return;
    }

    this.botSettingsService.updateTimeout(timeout.name, newValue).subscribe({
      next: () => {
        timeout.valueSeconds = newValue;
        this.showSuccess(`${timeout.name} updated to ${newValue} seconds`);
      },
      error: (err) => {
        console.error(`Error updating ${timeout.name}:`, err);
        this.error.set(`Failed to update ${timeout.name}`);
        input.value = timeout.valueSeconds.toString();
      }
    });
  }

  showSuccess(message: string) {
    this.successMessage.set(message);
    setTimeout(() => this.successMessage.set(null), 3000);
  }

  logout() {
    this.authService.logout();
  }
}
