import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
   baseUrl = environment.apiUrl;
  constructor(private http :HttpClient) { }

  getMembers()
  {
    return this.http.get<Member[]>(this.baseUrl + 'User' )
  }

  getMember(userName : string)
  {
    return this.http.get<Member>(this.baseUrl + 'User/' + userName )
  }


}
