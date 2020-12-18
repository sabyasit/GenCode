import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';

import { OpenApiService } from '../services/openapi';

@Component({
  selector: 'app-openapi',
  templateUrl: './openapi.component.html',
  styleUrls: ['./openapi.component.css']
})
export class OpenapiComponent implements OnInit {
  @ViewChild('editor') editor: any;
  public list: any = [];
  filename: string = '';
  projName: string;

  constructor(private openApiService: OpenApiService, private router: Router) {
    this.projName = this.router.getCurrentNavigation()?.extras?.state?.proj;
  }

  ngOnInit(): void {

  }

  renderBuilder(item: any) {
    this.router.navigateByUrl('/builder', { state: { item: item, proj: this.projName }});
  }

  getValue() {
    this.openApiService.postOpenApi(this.editor.value).subscribe((res) => {
      this.list = res;
    });
  }

  readURL($event: any) {
    this.filename = $event.target.files[0].name;
    var file: File = $event.target.files[0];
    var myReader: FileReader = new FileReader();
    myReader.onloadend = (e) => {
      this.openApiService.postOpenApi(myReader.result as string).subscribe((res) => {
        this.list = res;
      });
    }
    myReader.readAsText(file);
  }
}
