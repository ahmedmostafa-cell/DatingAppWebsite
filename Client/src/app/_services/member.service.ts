import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userparams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
   baseUrl = environment.apiUrl;
   members:Member[]=[];
   memberCash = new Map();
   userparamse :UserParams | undefined;
   user:User| undefined;
  constructor(private http :HttpClient , private accountService:AccountService)
  {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:user=> {
        if(user)
        {
          this.userparamse = new UserParams(user);
          this.user = user;
        }

      }
    })

  }

  getUserParams()
  {
    return this.userparamse;
  }

  setUserParams(params:UserParams)
  {
    this.userparamse = params;
  }

  resetUserParams()
  {
    if(this.user)
    {
      this.userparamse = new UserParams(this.user);
      return this.userparamse;
    }
    return;
  }
  getMembers(userparams:UserParams)
  {
    const response = this.memberCash.get(Object.values(userparams).join('-'));
    if(response) return of(response);
    let params = getPaginationHeaders(userparams.pageNumber , userparams.pageSize);

    params = params.append('minAge' , userparams.minAge);
    params = params.append('maxAge' , userparams.maxAge);

    params = params.append('gender' , userparams.gender);


    params = params.append('orderBy' , userparams.orderBy);
    return getPaginatedResult<Member[]>(this.baseUrl + 'User', params , this.http).pipe(
      map(response=> {
        this.memberCash.set(Object.values(userparams).join('-'),response);
        return response;
      })
    )
  }



  getMember(userName : string)
  {
   const member =[...this.memberCash.values()].reduce((arr,elem)=> arr.concat(elem.result),[]).find((member:Member)=>member.userName===userName);
    if(member) return of(member);
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

  setMainPhoto(photoId:number)
  {
    return this.http.put(this.baseUrl + 'User/set-main-photo/' + photoId , {});
  }

  deletePhoto(photoId:number)
  {
    return this.http.delete(this.baseUrl + 'User/delete-photo/' + photoId);
  }
  addLike(username:string)
  {
    return this.http.post(this.baseUrl + 'likes/' + username ,{});
  }
  getLikes(predicate:string , pageNumber:number , pageSize:number)
  {

    let params = getPaginationHeaders(pageNumber , pageSize);
    params=params.append('predicate' , predicate);
    return getPaginatedResult<Member[]>(this.baseUrl + 'likes'  , params , this.http);
  }

}
