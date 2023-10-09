import { ResolveFn } from '@angular/router';
import { Member } from '../_models/member';
import { inject } from '@angular/core';
import { MemberService } from '../_services/member.service';

export const memberDetailedResolver: ResolveFn<Member> = (route, state) => {
  const memberSrvice =inject(MemberService);


  return memberSrvice.getMember(route.paramMap.get('username')!)
};
