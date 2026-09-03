import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Shell } from './layout/shell/shell';

@Component({
  selector: 'app-root',
  imports: [Shell],
  template: '<app-shell />'
})
export class App {
  protected readonly title = signal('school-management-web');
}
