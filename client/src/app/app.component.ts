import { Component, OnInit  } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit { 
  currentRoute: string = '';

  constructor(private router: Router) {
      
  }

  ngOnInit() {
    this.router.events.subscribe((event: any) => {
      switch (true) {
        case event instanceof NavigationEnd: {
          this.currentRoute = this.router.url;
          break;
        }
      }
    });
  }
  
}
