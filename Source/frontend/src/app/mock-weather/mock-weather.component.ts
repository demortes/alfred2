import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-mock-weather',
  templateUrl: './mock-weather.component.html',
  styleUrls: ['./mock-weather.component.less']
})
export class MockWeatherComponent implements OnInit {
  public weather = {};
  
  /**
   *
   */
  constructor(private httpClient: HttpClient) {
    
    
  }

  ngOnInit(): void {
    this.httpClient.get(environment.apiUrl + "/weather").subscribe(response => {
      this.weather = response;
    })
  }

}
