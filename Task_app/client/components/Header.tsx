import Link from 'next/link';

export default function Header() {
  return (
    <header className="bg-blue-600 shadow">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          <div className="flex-shrink-0">
            <span className="text-white text-2xl font-bold">MyApp</span>
          </div>
          <nav>
            <ul className="flex space-x-8">
              <li>
                <Link className="text-white hover:text-blue-200 transition-colors" href="/register">
                    Register
                </Link>
              </li>
              <li>
                <Link className="text-white hover:text-blue-200 transition-colors" href="/task_manager">
                    Manager
                </Link>
              </li>
              <li>
                <Link className="text-white hover:text-blue-200 transition-colors" href="/userprofile">
                    Profile
                </Link>
              </li>
            </ul>
          </nav>
        </div>
      </div>
    </header>
  );
}
