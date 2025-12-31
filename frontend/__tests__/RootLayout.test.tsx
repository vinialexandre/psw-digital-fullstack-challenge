import { render } from '@testing-library/react';
import RootLayout, { metadata } from '@/app/layout';

describe('RootLayout', () => {
  it('renderiza children corretamente', () => {
    const { getByText } = render(
      <RootLayout>
        <div>Test Content</div>
      </RootLayout>
    );

    expect(getByText('Test Content')).toBeInTheDocument();
  });

  it('possui metadata correto', () => {
    expect(metadata.title).toBe('API de Feriados - Feriados Brasileiros');
    expect(metadata.description).toBe('Visualize e filtre os feriados brasileiros por ano');
  });
});
