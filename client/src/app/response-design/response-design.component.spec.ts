import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResponseDesignComponent } from './response-design.component';

describe('ResponseDesignComponent', () => {
  let component: ResponseDesignComponent;
  let fixture: ComponentFixture<ResponseDesignComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ResponseDesignComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResponseDesignComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
