import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "API de Feriados - Feriados Brasileiros",
  description: "Visualize e filtre os feriados brasileiros por ano",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="pt-BR">
      <body className="antialiased">
        {children}
      </body>
    </html>
  );
}

