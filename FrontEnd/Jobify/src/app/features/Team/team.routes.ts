import { Routes } from "@angular/router";
import { MembersTabComponent } from "./Components/members-tab/members-tab.component";
import { TeamLayoutComponent } from "./Components/team-layout/team-layout.component";
import { JoinRequestsTabComponent } from "./Components/join-requests-tab/join-requests-tab.component";
import { ActivityTabComponent } from "./Components/activity-tab/activity-tab.component";


export const Team_ROUTES: Routes = [

{
  path: 'team',
  component: TeamLayoutComponent,
  children: [
    { path: '', redirectTo: 'members', pathMatch: 'full' },
    { path: 'members', component: MembersTabComponent },
    { path: 'join-requests', component: JoinRequestsTabComponent },
    { path: 'activity', component: ActivityTabComponent }

    // 'join-requests' will be added when we build that tab
  ]
}
];