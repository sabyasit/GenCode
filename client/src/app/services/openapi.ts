import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class OpenApiService {

  constructor(private http: HttpClient) {
    
  }

  postOpenApi(data: string): Observable<any> {
    return this.http.post('api/openapi', {data: data});
  }

  GenerateCode(data: any): Observable<any> {
    return this.http.post('api/generate', data);
  }
}
