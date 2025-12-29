import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import LoginPage from '@/app/login/page';
import { authService } from '@/lib/api';
import { useRouter } from 'next/navigation';

jest.mock('next/navigation', () => ({
  useRouter: jest.fn(),
}));

jest.mock('@/lib/api', () => ({
  authService: {
    login: jest.fn(),
  },
}));

const mockLogin = authService.login as jest.Mock;
const mockUseRouter = useRouter as jest.Mock;

describe('LoginPage', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockUseRouter.mockReturnValue({ push: jest.fn() });
  });

  function fillForm(username: string, password: string) {
    fireEvent.change(screen.getByPlaceholderText('admin'), {
      target: { value: username },
    });

    fireEvent.change(screen.getByPlaceholderText('admin123'), {
      target: { value: password },
    });
  }

  it('realiza login com sucesso e redireciona', async () => {
    const push = jest.fn();
    mockUseRouter.mockReturnValue({ push });

    mockLogin.mockResolvedValue({
      success: true,
      data: { username: 'admin', expiresAt: '2025-01-01' },
      message: '',
      totalRecords: 0,
    });

    render(<LoginPage />);

    fillForm('admin', 'admin123');

    fireEvent.click(screen.getByText('Entrar'));

    await waitFor(() => {
      expect(mockLogin).toHaveBeenCalled();
    });

    expect(push).toHaveBeenCalledWith('/');
  });

  it('exibe mensagem de erro quando API retorna falha', async () => {
    mockLogin.mockResolvedValue({
      success: false,
      data: null,
      message: 'Credenciais invalidas',
      totalRecords: 0,
    });

    render(<LoginPage />);

    fillForm('admin', 'errada');
    fireEvent.click(screen.getByText('Entrar'));

    await waitFor(() => {
      expect(screen.getByText('Credenciais invalidas')).toBeInTheDocument();
    });
  });

  it('exibe mensagem de erro quando login lanca excecao', async () => {
    mockLogin.mockRejectedValue(new Error('Falha no servidor'));

    render(<LoginPage />);

    fillForm('admin', 'admin123');
    fireEvent.click(screen.getByText('Entrar'));

    await waitFor(() => {
      expect(screen.getByText('Falha no servidor')).toBeInTheDocument();
    });
  });

  it('valida campos obrigatorios', async () => {
    render(<LoginPage />);

    fireEvent.click(screen.getByText('Entrar'));

    await waitFor(() => {
      expect(screen.getByText('Usuário é obrigatório')).toBeInTheDocument();
      expect(screen.getByText('Senha é obrigatória')).toBeInTheDocument();
    });
  });
});
