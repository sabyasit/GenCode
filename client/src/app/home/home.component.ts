import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  projName: string = '';

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  createProjAndNavigate() {
    this.router.navigateByUrl("/openapi", { state: {proj: this.projName }});
  }

}
