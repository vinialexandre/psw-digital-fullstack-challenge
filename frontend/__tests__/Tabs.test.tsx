import { render, screen, fireEvent } from '@testing-library/react';
import { Tabs, Tab } from '@/components/Tabs';

describe('Tabs', () => {
  const mockTabs: Tab[] = [
    { id: 'tab1', label: 'Tab 1', content: <div>Content 1</div> },
    { id: 'tab2', label: 'Tab 2', content: <div>Content 2</div> },
    { id: 'tab3', label: 'Tab 3', content: <div>Content 3</div> },
  ];

  it('renders all tab labels', () => {
    render(<Tabs tabs={mockTabs} />);
    
    expect(screen.getByText('Tab 1')).toBeInTheDocument();
    expect(screen.getByText('Tab 2')).toBeInTheDocument();
    expect(screen.getByText('Tab 3')).toBeInTheDocument();
  });

  it('displays first tab content by default', () => {
    render(<Tabs tabs={mockTabs} />);
    
    expect(screen.getByText('Content 1')).toBeInTheDocument();
    expect(screen.queryByText('Content 2')).not.toBeInTheDocument();
    expect(screen.queryByText('Content 3')).not.toBeInTheDocument();
  });

  it('displays specified default tab content', () => {
    render(<Tabs tabs={mockTabs} defaultTab="tab2" />);
    
    expect(screen.queryByText('Content 1')).not.toBeInTheDocument();
    expect(screen.getByText('Content 2')).toBeInTheDocument();
    expect(screen.queryByText('Content 3')).not.toBeInTheDocument();
  });

  it('switches content when clicking different tab', () => {
    render(<Tabs tabs={mockTabs} />);
    
    expect(screen.getByText('Content 1')).toBeInTheDocument();
    
    const tab2Button = screen.getByText('Tab 2');
    fireEvent.click(tab2Button);
    
    expect(screen.queryByText('Content 1')).not.toBeInTheDocument();
    expect(screen.getByText('Content 2')).toBeInTheDocument();
  });

  it('applies active styles to current tab', () => {
    render(<Tabs tabs={mockTabs} />);
    
    const tab1Button = screen.getByText('Tab 1');
    expect(tab1Button).toHaveClass('border-blue-500');
    expect(tab1Button).toHaveClass('text-blue-600');
  });
});

