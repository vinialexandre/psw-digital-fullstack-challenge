import { render, screen, fireEvent, waitFor } from '@testing-library/react';
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
      expect(screen.getByText('Feriados Brasileiros 2025')).toBeInTheDocument();
    });

    expect(screen.getByText('Ano Novo')).toBeInTheDocument();
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
      expect(screen.getByText('Sair')).toBeInTheDocument();
    });

    fireEvent.click(screen.getByText('Sair'));

    await waitFor(() => {
      expect(mockLogout).toHaveBeenCalled();
    });

    expect(push).toHaveBeenCalledWith('/login');
  });
});
