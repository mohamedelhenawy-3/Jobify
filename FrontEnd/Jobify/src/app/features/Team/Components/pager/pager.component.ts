import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-pager',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pager.component.html',
  styleUrl: './pager.component.css'
})
export class PagerComponent {
  @Input() page = 1;
  @Input() totalPages = 1;
  @Input() totalCount = 0;
  @Output() pageChange = new EventEmitter<number>();
}
