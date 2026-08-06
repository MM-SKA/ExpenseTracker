import { Component, ElementRef, ViewChild, AfterViewChecked } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface ChatMessage {
  role: 'ai' | 'user';
  content: string;
}

@Component({
  selector: 'app-ai-chat',
  imports: [FormsModule],
  templateUrl: './ai-chat.html',
  styleUrl: './ai-chat.css',
})
export class AiChat implements AfterViewChecked {
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef<HTMLDivElement>;

  messages: ChatMessage[] = [];
  inputText = '';
  isTyping = false;

  private shouldScroll = false;

  ngAfterViewChecked(): void {
    if (this.shouldScroll) {
      this.scrollToBottom();
      this.shouldScroll = false;
    }
  }

  send(): void {
    const text = this.inputText.trim();
    if (!text || this.isTyping) return;

    this.messages.push({ role: 'user', content: text });
    this.inputText = '';
    this.isTyping = true;
    this.shouldScroll = true;

    // Simulate AI response (replace with real API call)
    setTimeout(() => {
      this.messages.push({
        role: 'ai',
        content: this.getMockResponse(text),
      });
      this.isTyping = false;
      this.shouldScroll = true;
    }, 1200 + Math.random() * 800);
  }

  sendChip(text: string): void {
    this.inputText = text;
    this.send();
  }

  onEnter(event: Event): void {
    const ke = event as KeyboardEvent;
    if (!ke.shiftKey) {
      ke.preventDefault();
      this.send();
    }
  }

  private scrollToBottom(): void {
    try {
      const el = this.messagesContainer?.nativeElement;
      if (el) el.scrollTop = el.scrollHeight;
    } catch {
      // ignore
    }
  }

  private getMockResponse(input: string): string {
    const lower = input.toLowerCase();
    if (lower.includes('summary') || lower.includes('month')) {
      return "📊 This month you've spent $2,340 across 8 categories. Food & Dining accounts for the largest share at 32%, followed by Transport at 18%. You're 12% under your monthly budget — great work!";
    }
    if (lower.includes('overspend') || lower.includes('alert')) {
      return '⚠️ You\'re overspending in "Entertainment" — currently at 142% of your set budget. Consider reducing streaming subscriptions or dining out to get back on track.';
    }
    if (lower.includes('saving') || lower.includes('tip')) {
      return '💡 Based on your patterns: (1) You spend ~$180/month on coffee — brewing at home could save $100+. (2) Your subscriptions total $65/month — consider auditing unused ones. (3) Grocery spending peaks mid-week; batch shopping on weekends could reduce impulse buys.';
    }
    if (lower.includes('top') || lower.includes('biggest')) {
      return '🔝 Your top 5 expenses this month:\n1. Rent — $1,200\n2. Groceries — $380\n3. Dining Out — $210\n4. Transport — $145\n5. Entertainment — $98';
    }
    return "I'm analyzing your financial data... To give you the most accurate insights, try asking about specific categories, date ranges, or budgets. I can help with spending analysis, savings tips, and budget recommendations! 💬";
  }
}
