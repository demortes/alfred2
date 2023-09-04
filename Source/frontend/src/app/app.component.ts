import { Component, OnInit } from '@angular/core';
import { MsalService } from '@azure/msal-angular';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent implements OnInit {
  title = 'Alfred';
  /**
   *
   */
  constructor(private msalService: MsalService) { }
  ngOnInit(): void {
    this.msalService.loginPopup();
  }
}
