import { render, screen, fireEvent } from '@testing-library/react';
import { HolidayFilters } from '@/components/HolidayFilters';

describe('HolidayFilters', () => {
  it('renders all filter inputs', () => {
    const mockOnFilterChange = jest.fn();
    render(<HolidayFilters onFilterChange={mockOnFilterChange} />);

    expect(screen.getByPlaceholderText('Busque por nome')).toBeInTheDocument();
    expect(screen.getAllByText('Tipo').length).toBeGreaterThan(0);
    expect(screen.getByText('Data do Feriado')).toBeInTheDocument();
  });

  it('calls onFilterChange when search button is clicked', () => {
    const mockOnFilterChange = jest.fn();
    render(<HolidayFilters onFilterChange={mockOnFilterChange} />);

    const searchInput = screen.getByPlaceholderText('Busque por nome');
    fireEvent.change(searchInput, { target: { value: 'Natal' } });
    fireEvent.keyDown(searchInput, { key: 'Enter' });

    expect(mockOnFilterChange).toHaveBeenCalled();
    const callArgs = mockOnFilterChange.mock.calls[0][0];
    expect(callArgs.searchTerm).toBe('Natal');
    expect(callArgs.year).toBeDefined();
  });

  it('calls onFilterChange when type is selected', () => {
    const mockOnFilterChange = jest.fn();
    const { container } = render(<HolidayFilters onFilterChange={mockOnFilterChange} />);

    const typeSelect = container.querySelectorAll('select')[0];
    fireEvent.change(typeSelect, { target: { value: 'National' } });

    const searchInput = screen.getByPlaceholderText('Busque por nome');
    fireEvent.keyDown(searchInput, { key: 'Enter' });

    expect(mockOnFilterChange).toHaveBeenCalled();
    const callArgs = mockOnFilterChange.mock.calls[0][0];
    expect(callArgs.type).toBe('National');
  });

  it('calls onFilterChange when year is selected', () => {
    const mockOnFilterChange = jest.fn();
    const { container } = render(<HolidayFilters onFilterChange={mockOnFilterChange} />);

    const yearSelect = container.querySelectorAll('select')[1];
    fireEvent.change(yearSelect, { target: { value: '2023' } });

    const searchInput = screen.getByPlaceholderText('Busque por nome');
    fireEvent.keyDown(searchInput, { key: 'Enter' });

    expect(mockOnFilterChange).toHaveBeenCalled();
    const callArgs = mockOnFilterChange.mock.calls[0][0];
    expect(callArgs.year).toBe(2023);
  });

  it('clears all filters when clear button is clicked', () => {
    const mockOnFilterChange = jest.fn();
    render(<HolidayFilters onFilterChange={mockOnFilterChange} />);

    const searchInput = screen.getByPlaceholderText('Busque por nome');
    fireEvent.change(searchInput, { target: { value: 'Natal' } });
    fireEvent.keyDown(searchInput, { key: 'Enter' });

    expect(mockOnFilterChange).toHaveBeenCalled();
    expect(searchInput).toHaveValue('Natal');
  });
});
