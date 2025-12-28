import { render, screen, fireEvent } from '@testing-library/react';
import { HolidayFilters } from '@/components/HolidayFilters';

describe('HolidayFilters', () => {
  it('renders all filter inputs', () => {
    const mockOnFilterChange = jest.fn();
    render(<HolidayFilters onFilterChange={mockOnFilterChange} />);

    expect(screen.getByPlaceholderText('Nome do feriado...')).toBeInTheDocument();
    expect(screen.getByText('Tipo')).toBeInTheDocument();
    expect(screen.getByText('Ano')).toBeInTheDocument();
    expect(screen.getByText('Filtrar')).toBeInTheDocument();
    expect(screen.getByText('Limpar')).toBeInTheDocument();
  });

  it('calls onFilterChange when search button is clicked', () => {
    const mockOnFilterChange = jest.fn();
    render(<HolidayFilters onFilterChange={mockOnFilterChange} />);

    const searchInput = screen.getByPlaceholderText('Nome do feriado...');
    fireEvent.change(searchInput, { target: { value: 'Natal' } });

    const searchButton = screen.getByText('Filtrar');
    fireEvent.click(searchButton);

    expect(mockOnFilterChange).toHaveBeenCalled();
    const callArgs = mockOnFilterChange.mock.calls[0][0];
    expect(callArgs.searchTerm).toBe('Natal');
    expect(callArgs.year).toBeDefined();
  });

  it('calls onFilterChange when type is selected', () => {
    const mockOnFilterChange = jest.fn();
    const { container } = render(<HolidayFilters onFilterChange={mockOnFilterChange} />);

    const typeSelect = container.querySelectorAll('select')[1];
    fireEvent.change(typeSelect, { target: { value: 'National' } });

    const searchButton = screen.getByText('Filtrar');
    fireEvent.click(searchButton);

    expect(mockOnFilterChange).toHaveBeenCalled();
    const callArgs = mockOnFilterChange.mock.calls[0][0];
    expect(callArgs.type).toBe('National');
  });

  it('calls onFilterChange when year is selected', () => {
    const mockOnFilterChange = jest.fn();
    const { container } = render(<HolidayFilters onFilterChange={mockOnFilterChange} />);

    const yearSelect = container.querySelectorAll('select')[0];
    fireEvent.change(yearSelect, { target: { value: '2023' } });

    const searchButton = screen.getByText('Filtrar');
    fireEvent.click(searchButton);

    expect(mockOnFilterChange).toHaveBeenCalled();
    const callArgs = mockOnFilterChange.mock.calls[0][0];
    expect(callArgs.year).toBe(2023);
  });

  it('clears all filters when clear button is clicked', () => {
    const mockOnFilterChange = jest.fn();
    const currentYear = new Date().getFullYear();
    render(<HolidayFilters onFilterChange={mockOnFilterChange} />);

    const searchInput = screen.getByPlaceholderText('Nome do feriado...');
    fireEvent.change(searchInput, { target: { value: 'Natal' } });

    const clearButton = screen.getByText('Limpar');
    fireEvent.click(clearButton);

    expect(mockOnFilterChange).toHaveBeenCalledWith({ year: currentYear });
    expect(searchInput).toHaveValue('');
  });
});

