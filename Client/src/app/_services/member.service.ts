import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userparams';
import { AccountService } from './account.service';
import { User } from '../_models/user';

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
    let params = this.getPaginationHeaders(userparams.pageNumber , userparams.pageSize);

    params = params.append('minAge' , userparams.minAge);
    params = params.append('maxAge' , userparams.maxAge);

    params = params.append('gender' , userparams.gender);


    params = params.append('orderBy' , userparams.orderBy);
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'User', params).pipe(
      map(response=> {
        this.memberCash.set(Object.values(userparams).join('-'),response);
        return response;
      })
    )
  }

  private getPaginatedResult<T>(url:string , params: HttpParams) {
    const paginatedResult :PaginatedResult<T> = new PaginatedResult<T>;
    return this.http.get<T>(this.baseUrl + 'User', { observe: 'response', params }).pipe(
      map(response => {
        if (response.body) {
          paginatedResult.result = response.body;
        }
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          paginatedResult.pagination = JSON.parse(pagination);
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber:number , pageSize:number) {
    let params = new HttpParams();

      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);

    return params;
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


}
