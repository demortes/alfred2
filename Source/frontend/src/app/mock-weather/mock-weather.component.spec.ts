import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MockWeatherComponent } from './mock-weather.component';

describe('MockWeatherComponent', () => {
  let component: MockWeatherComponent;
  let fixture: ComponentFixture<MockWeatherComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [MockWeatherComponent, HttpClientTestingModule]
    });
    fixture = TestBed.createComponent(MockWeatherComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
