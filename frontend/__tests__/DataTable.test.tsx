import { render, screen, fireEvent } from '@testing-library/react';
import { DataTable, Column } from '@/components/DataTable';

interface TestData extends Record<string, unknown> {
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
    render(<DataTable<TestData> data={mockData} columns={columns} />);
    
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
      <DataTable<TestData>
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
    const { container } = render(
      <DataTable<TestData>
        data={mockData}
        columns={columns}
        currentSortField="name"
        currentSortDescending={false}
      />
    );

    const headers = container.querySelectorAll('button');
    expect(headers.length).toBeGreaterThan(0);
  });

  it('renders custom cell content when render function is provided', () => {
    const customColumns: Column<TestData>[] = [
      {
        key: 'value',
        header: 'Value',
        render: (value) => <span>$ {value}</span>,
      },
    ];

    render(<DataTable<TestData> data={mockData} columns={customColumns} />);
    
    expect(screen.getByText('$ 100')).toBeInTheDocument();
    expect(screen.getByText('$ 200')).toBeInTheDocument();
    expect(screen.getByText('$ 300')).toBeInTheDocument();
  });

  it('handles items without id or date using index', () => {
    const dataWithoutIds = [{ name: 'Test', value: 100 }] as TestData[];
    render(<DataTable<TestData> data={dataWithoutIds} columns={columns} />);
    expect(screen.getByText('Test')).toBeInTheDocument();
  });

  it('handles name as object in getItemName', () => {
    const dataWithObjectName = [{ id: 1, name: { first: 'John' }, value: 100 }] as unknown as TestData[];
    render(<DataTable<TestData> data={dataWithObjectName} columns={columns} />);
    expect(screen.getByLabelText(/ver detalhes do feriado/i)).toBeInTheDocument();
  });

  it('handles name as null or undefined in getItemName', () => {
    const dataWithNullName = [{ id: 1, name: null, value: 100 }] as unknown as TestData[];
    render(<DataTable<TestData> data={dataWithNullName} columns={columns} />);
    expect(screen.getByLabelText(/ver detalhes do feriado/i)).toBeInTheDocument();
  });

  it('toggles sort descending when clicking same field', () => {
    const mockOnSort = jest.fn();
    render(
      <DataTable<TestData>
        data={mockData}
        columns={columns}
        onSort={mockOnSort}
        currentSortField="id"
        currentSortDescending={true}
      />
    );
    
    const idHeader = screen.getByText('ID');
    fireEvent.click(idHeader);
    
    expect(mockOnSort).toHaveBeenCalledWith('id', false);
  });
});
