import { render, screen, fireEvent } from '@testing-library/react';
import { DataTable, Column } from '@/components/DataTable';

interface TestData {
  id: number;
  name: string;
  value: number;
}

describe('DataTable', () => {
  const mockData: TestData[] = [
    { id: 1, name: 'Item 1', value: 100 },
    { id: 2, name: 'Item 2', value: 200 },
    { id: 3, name: 'Item 3', value: 300 },
  ];

  const columns: Column<TestData>[] = [
    { key: 'id', header: 'ID', sortable: true },
    { key: 'name', header: 'Name', sortable: true },
    { key: 'value', header: 'Value', sortable: false },
  ];

  it('renders table with data', () => {
    render(<DataTable data={mockData} columns={columns} />);
    
    expect(screen.getByText('ID')).toBeInTheDocument();
    expect(screen.getByText('Name')).toBeInTheDocument();
    expect(screen.getByText('Value')).toBeInTheDocument();
    
    expect(screen.getByText('Item 1')).toBeInTheDocument();
    expect(screen.getByText('Item 2')).toBeInTheDocument();
    expect(screen.getByText('Item 3')).toBeInTheDocument();
  });

  it('calls onSort when clicking sortable column', () => {
    const mockOnSort = jest.fn();
    render(
      <DataTable
        data={mockData}
        columns={columns}
        onSort={mockOnSort}
        currentSortField="id"
        currentSortDescending={false}
      />
    );
    
    const idHeader = screen.getByText('ID');
    fireEvent.click(idHeader);
    
    expect(mockOnSort).toHaveBeenCalledWith('id', true);
  });

  it('displays sort indicator for current sort field', () => {
    render(
      <DataTable
        data={mockData}
        columns={columns}
        currentSortField="name"
        currentSortDescending={false}
      />
    );
    
    expect(screen.getByText('↑')).toBeInTheDocument();
  });

  it('renders custom cell content when render function is provided', () => {
    const customColumns: Column<TestData>[] = [
      {
        key: 'value',
        header: 'Value',
        render: (value) => <span>$ {value}</span>,
      },
    ];

    render(<DataTable data={mockData} columns={customColumns} />);
    
    expect(screen.getByText('$ 100')).toBeInTheDocument();
    expect(screen.getByText('$ 200')).toBeInTheDocument();
    expect(screen.getByText('$ 300')).toBeInTheDocument();
  });
});

