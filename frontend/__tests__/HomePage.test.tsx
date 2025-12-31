import { render, screen, fireEvent, waitFor, within } from '@testing-library/react';
import HomePage from '@/app/page';
import { useHolidays } from '@/hooks/useHolidays';
import { authService } from '@/lib/api';
import { useRouter } from 'next/navigation';

jest.mock('next/navigation', () => ({
  useRouter: jest.fn(),
}));

jest.mock('@/hooks/useHolidays', () => ({
  useHolidays: jest.fn(),
}));

jest.mock('@/lib/api', () => ({
  authService: {
    logout: jest.fn(),
  },
}));

const mockUseHolidays = useHolidays as jest.Mock;
const mockUseRouter = useRouter as jest.Mock;
const mockLogout = authService.logout as jest.Mock;

function createHookState(overrides?: Partial<ReturnType<typeof useHolidays>>) {
  return {
    holidays: [],
    loading: false,
    error: null,
    totalRecords: 0,
    filter: { year: 2025, sortBy: 'date', sortDescending: false },
    setFilter: jest.fn(),
    refetch: jest.fn(),
    ...overrides,
  } as any;
}

describe('HomePage', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockUseRouter.mockReturnValue({ push: jest.fn() });
  });

  it('exibe estado de carregamento', async () => {
    mockUseHolidays.mockReturnValue(
      createHookState({ loading: true })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByText('Carregando feriados...')).toBeInTheDocument();
    });
  });

  it('exibe mensagem de erro quando houver erro', async () => {
    mockUseHolidays.mockReturnValue(
      createHookState({ error: 'Falha ao carregar' })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByText('Falha ao carregar')).toBeInTheDocument();
    });
  });

  it('exibe mensagem de lista vazia quando nao ha feriados', async () => {
    mockUseHolidays.mockReturnValue(
      createHookState({ holidays: [], totalRecords: 0 })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByText('Nenhum dado encontrado')).toBeInTheDocument();
    });
  });

  it('renderiza tabela quando ha feriados', async () => {
    mockUseHolidays.mockReturnValue(
      createHookState({
        holidays: [
          { date: '2025-01-01', name: 'Ano Novo', type: 'National' },
        ],
        totalRecords: 1,
      })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByText('Ano Novo')).toBeInTheDocument();
    });

    expect(screen.getAllByText('Nacional').length).toBeGreaterThan(0);
  });

  it('chama logout e redireciona para login', async () => {
    const push = jest.fn();
    mockUseRouter.mockReturnValue({ push });

    mockUseHolidays.mockReturnValue(
      createHookState({ holidays: [], totalRecords: 0 })
    );

    mockLogout.mockResolvedValue(undefined);

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByText('MEUS FERIADOS')).toBeInTheDocument();
    });

    const logoutButton = screen.getByRole('button', { name: /logout|sair/i });
    fireEvent.click(logoutButton);

    await waitFor(() => {
      expect(mockLogout).toHaveBeenCalled();
    });

    expect(push).toHaveBeenCalledWith('/login');
  });

  it('permite buscar feriados por nome', async () => {
    const setFilter = jest.fn();
    mockUseHolidays.mockReturnValue(
      createHookState({ holidays: [], totalRecords: 0, setFilter })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByPlaceholderText('Busque por nome')).toBeInTheDocument();
    });

    const searchInput = screen.getByPlaceholderText('Busque por nome');
    fireEvent.change(searchInput, { target: { value: 'Natal' } });

    const searchButton = screen.getByRole('button', { name: /buscar feriados/i });
    fireEvent.click(searchButton);

    expect(setFilter).toHaveBeenCalled();
  });

  it('permite limpar busca', async () => {
    const setFilter = jest.fn();
    mockUseHolidays.mockReturnValue(
      createHookState({ holidays: [], totalRecords: 0, setFilter })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByPlaceholderText('Busque por nome')).toBeInTheDocument();
    });

    const searchInput = screen.getByPlaceholderText('Busque por nome');
    fireEvent.change(searchInput, { target: { value: 'Natal' } });

    await waitFor(() => {
      const buttons = screen.getAllByRole('button');
      const clearButton = buttons.find(btn => btn.querySelector('svg path[d*="M6 18L18 6M6 6l12 12"]'));
      if (clearButton) {
        fireEvent.click(clearButton);
      }
    });

    expect(setFilter).toHaveBeenCalled();
  });

  it('permite ordenar feriados', async () => {
    const setFilter = jest.fn();
    mockUseHolidays.mockReturnValue(
      createHookState({ holidays: [], totalRecords: 0, setFilter })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getAllByLabelText('Ordenar por').length).toBeGreaterThan(0);
    });

    const sortSelects = screen.getAllByLabelText('Ordenar por');
    fireEvent.change(sortSelects[0], { target: { value: 'name' } });

    expect(setFilter).toHaveBeenCalled();
  });

  it('permite filtrar por tipo', async () => {
    const setFilter = jest.fn();
    mockUseHolidays.mockReturnValue(
      createHookState({ holidays: [], totalRecords: 0, setFilter })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByLabelText('Filtrar por tipo')).toBeInTheDocument();
    });

    const typeSelect = screen.getByLabelText('Filtrar por tipo');
    fireEvent.change(typeSelect, { target: { value: 'National' } });

    expect(typeSelect).toHaveValue('National');
  });

  it('permite filtrar por ano', async () => {
    const setFilter = jest.fn();
    mockUseHolidays.mockReturnValue(
      createHookState({ holidays: [], totalRecords: 0, setFilter })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByLabelText('Filtrar por ano')).toBeInTheDocument();
    });

    const yearSelect = screen.getByLabelText('Filtrar por ano');
    fireEvent.change(yearSelect, { target: { value: '2024' } });

    expect(yearSelect).toHaveValue('2024');
  });

  it('abre modal ao clicar em feriado', async () => {
    mockUseHolidays.mockReturnValue(
      createHookState({
        holidays: [
          { date: '2025-01-01', name: 'Ano Novo', type: 'National' },
        ],
        totalRecords: 1,
      })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByText('Ano Novo')).toBeInTheDocument();
    });

    const holidayRow = screen.getByRole('button', { name: /ver detalhes do feriado ano novo/i });
    fireEvent.click(holidayRow);

    await waitFor(() => {
      expect(screen.getByText('Detalhes do Feriado')).toBeInTheDocument();
    });
  });

  it('fecha modal ao clicar no botao fechar', async () => {
    mockUseHolidays.mockReturnValue(
      createHookState({
        holidays: [
          { date: '2025-01-01', name: 'Ano Novo', type: 'National' },
        ],
        totalRecords: 1,
      })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByText('Ano Novo')).toBeInTheDocument();
    });

    const holidayRow = screen.getByRole('button', { name: /ver detalhes do feriado ano novo/i });
    fireEvent.click(holidayRow);

    await waitFor(() => {
      expect(screen.getByText('Detalhes do Feriado')).toBeInTheDocument();
    });

    const modal = screen.getByText('Detalhes do Feriado').closest('div');
    if (modal && modal.parentElement) {
      const closeButtons = within(modal.parentElement).getAllByRole('button');
      const closeButton = closeButtons.find(btn => btn.querySelector('svg path[d*="M6 18L18 6M6 6l12 12"]'));
      if (closeButton) {
        fireEvent.click(closeButton);
      }
    }

    await waitFor(() => {
      expect(screen.queryByText('Detalhes do Feriado')).not.toBeInTheDocument();
    });
  });

  it('busca ao pressionar Enter no campo de busca', async () => {
    const setFilter = jest.fn();
    mockUseHolidays.mockReturnValue(
      createHookState({ holidays: [], totalRecords: 0, setFilter })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByPlaceholderText('Busque por nome')).toBeInTheDocument();
    });

    const searchInput = screen.getByPlaceholderText('Busque por nome');
    fireEvent.change(searchInput, { target: { value: 'Natal' } });
    fireEvent.keyDown(searchInput, { key: 'Enter', code: 'Enter' });

    expect(setFilter).toHaveBeenCalled();
  });

  it('trata erro no logout', async () => {
    const push = jest.fn();
    mockUseRouter.mockReturnValue({ push });
    mockUseHolidays.mockReturnValue(createHookState({ holidays: [], totalRecords: 0 }));
    mockLogout.mockRejectedValue(new Error('Logout failed'));

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByText('MEUS FERIADOS')).toBeInTheDocument();
    });

    const logoutButton = screen.getByRole('button', { name: /logout|sair/i });
    fireEvent.click(logoutButton);

    await waitFor(() => {
      expect(push).toHaveBeenCalledWith('/login');
    });
  });

  it('nao ordena quando campo vazio', async () => {
    const setFilter = jest.fn();
    mockUseHolidays.mockReturnValue(createHookState({ holidays: [], totalRecords: 0, setFilter }));

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getAllByLabelText('Ordenar por').length).toBeGreaterThan(0);
    });

    const sortSelects = screen.getAllByLabelText('Ordenar por');
    fireEvent.change(sortSelects[0], { target: { value: '' } });

    expect(setFilter).not.toHaveBeenCalled();
  });

  it('fecha modal ao clicar no overlay', async () => {
    mockUseHolidays.mockReturnValue(
      createHookState({
        holidays: [{ date: '2025-01-01', name: 'Ano Novo', type: 'National' }],
        totalRecords: 1,
      })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByText('Ano Novo')).toBeInTheDocument();
    });

    const holidayRow = screen.getByRole('button', { name: /ver detalhes do feriado ano novo/i });
    fireEvent.click(holidayRow);

    await waitFor(() => {
      expect(screen.getByText('Detalhes do Feriado')).toBeInTheDocument();
    });

    const overlay = screen.getByRole('button', { name: /fechar modal clicando fora/i });
    fireEvent.click(overlay);

    await waitFor(() => {
      expect(screen.queryByText('Detalhes do Feriado')).not.toBeInTheDocument();
    });
  });

  it('exibe tipo Municipal corretamente', async () => {
    mockUseHolidays.mockReturnValue(
      createHookState({
        holidays: [{ date: '2025-01-01', name: 'Feriado Municipal', type: 'Municipal' }],
        totalRecords: 1,
      })
    );

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByText('Feriado Municipal')).toBeInTheDocument();
    });

    const municipalBadges = screen.getAllByText('Municipal');
    expect(municipalBadges.length).toBeGreaterThan(0);
  });

  it('busca com ano selecionado', async () => {
    const setFilter = jest.fn();
    mockUseHolidays.mockReturnValue(createHookState({ holidays: [], totalRecords: 0, setFilter }));

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByLabelText('Filtrar por ano')).toBeInTheDocument();
    });

    const yearSelect = screen.getByLabelText('Filtrar por ano');
    fireEvent.change(yearSelect, { target: { value: '2024' } });

    const searchButton = screen.getByRole('button', { name: /buscar feriados/i });
    fireEvent.click(searchButton);

    expect(setFilter).toHaveBeenCalledWith(expect.objectContaining({ year: 2024 }));
  });

  it('limpa busca com ano selecionado', async () => {
    const setFilter = jest.fn();
    mockUseHolidays.mockReturnValue(createHookState({ holidays: [], totalRecords: 0, setFilter }));

    render(<HomePage />);

    await waitFor(() => {
      expect(screen.getByPlaceholderText('Busque por nome')).toBeInTheDocument();
    });

    const yearSelect = screen.getByLabelText('Filtrar por ano');
    fireEvent.change(yearSelect, { target: { value: '2024' } });

    const searchInput = screen.getByPlaceholderText('Busque por nome');
    fireEvent.change(searchInput, { target: { value: 'Natal' } });

    await waitFor(() => {
      const buttons = screen.getAllByRole('button');
      const clearButton = buttons.find(btn => btn.querySelector('svg path[d*="M6 18L18 6M6 6l12 12"]'));
      if (clearButton) {
        fireEvent.click(clearButton);
      }
    });

    expect(setFilter).toHaveBeenCalledWith(expect.objectContaining({ year: 2024, searchTerm: undefined }));
  });
});
