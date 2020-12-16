import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestDesignComponent } from './request-design.component';

describe('RequestDesignComponent', () => {
  let component: RequestDesignComponent;
  let fixture: ComponentFixture<RequestDesignComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RequestDesignComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RequestDesignComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
