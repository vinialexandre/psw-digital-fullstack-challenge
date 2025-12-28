import { render, screen, fireEvent } from '@testing-library/react';
import { HolidayFilters } from '@/components/HolidayFilters';

describe('HolidayFilters', () => {
  it('renders all filter inputs', () => {
    const mockOnFilterChange = jest.fn();
    render(<HolidayFilters onFilterChange={mockOnFilterChange} />);
    
    expect(screen.getByPlaceholderText('Holiday name...')).toBeInTheDocument();
    expect(screen.getByLabelText('Filter by Type')).toBeInTheDocument();
    expect(screen.getByLabelText('Filter by Date')).toBeInTheDocument();
  });

  it('calls onFilterChange when search button is clicked', () => {
    const mockOnFilterChange = jest.fn();
    render(<HolidayFilters onFilterChange={mockOnFilterChange} />);
    
    const searchInput = screen.getByPlaceholderText('Holiday name...');
    fireEvent.change(searchInput, { target: { value: 'Christmas' } });
    
    const searchButton = screen.getByText('🔍');
    fireEvent.click(searchButton);
    
    expect(mockOnFilterChange).toHaveBeenCalledWith({
      searchTerm: 'Christmas',
      type: undefined,
      date: undefined,
    });
  });

  it('calls onFilterChange when type is selected', () => {
    const mockOnFilterChange = jest.fn();
    render(<HolidayFilters onFilterChange={mockOnFilterChange} />);
    
    const typeSelect = screen.getByLabelText('Filter by Type');
    fireEvent.change(typeSelect, { target: { value: 'National' } });
    
    expect(mockOnFilterChange).toHaveBeenCalledWith({
      searchTerm: undefined,
      type: 'National',
      date: undefined,
    });
  });

  it('calls onFilterChange when date is selected', () => {
    const mockOnFilterChange = jest.fn();
    render(<HolidayFilters onFilterChange={mockOnFilterChange} />);
    
    const dateInput = screen.getByLabelText('Filter by Date');
    fireEvent.change(dateInput, { target: { value: '2025-12-25' } });
    
    expect(mockOnFilterChange).toHaveBeenCalledWith({
      searchTerm: undefined,
      type: undefined,
      date: '2025-12-25',
    });
  });

  it('clears all filters when clear button is clicked', () => {
    const mockOnFilterChange = jest.fn();
    render(<HolidayFilters onFilterChange={mockOnFilterChange} />);
    
    const searchInput = screen.getByPlaceholderText('Holiday name...');
    fireEvent.change(searchInput, { target: { value: 'Christmas' } });
    
    const clearButton = screen.getByText('Clear Filters');
    fireEvent.click(clearButton);
    
    expect(mockOnFilterChange).toHaveBeenCalledWith({});
    expect(searchInput).toHaveValue('');
  });
});

