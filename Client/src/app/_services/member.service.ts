import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
   baseUrl = environment.apiUrl;
   members:Member[]=[];
  constructor(private http :HttpClient) { }

  getMembers()
  {
    if(this.members.length >0) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl + 'User' ).pipe(
      map(members=> {
        this.members = members;
        return members;
      })
    )
  }

  getMember(userName : string)
  {
    const member = this.members.find(x=> x.userName ===userName);
    if(member) return of (member);
    return this.http.get<Member>(this.baseUrl + 'User/' + userName )
  }

  updateMember(member : Member)
  {
    return this.http.put(this.baseUrl + 'User' , member).pipe(
      map(()=> {
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member};
      })
    )
  }


}
