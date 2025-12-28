import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Holidays API - Brazilian Holidays 2025",
  description: "View and filter Brazilian holidays for 2025",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className="antialiased">
        {children}
      </body>
    </html>
  );
}

