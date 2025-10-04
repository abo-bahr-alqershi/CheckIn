interface PagePlaceholderProps {
  title: string;
  description: string;
  icon: string;
  comingSoon?: boolean;
}

const PagePlaceholder = ({ title, description, icon, comingSoon = false }: PagePlaceholderProps) => {
  return (
    <div className="bg-white rounded-lg shadow-sm p-8">
      <div className="text-center">
        <div className="text-6xl mb-4">{icon}</div>
        <h1 className="text-2xl font-bold text-gray-900 mb-2">{title}</h1>
        <p className="text-gray-600 mb-6">{description}</p>
        
        {comingSoon && (
          <div className="inline-flex items-center px-4 py-2 bg-blue-50 text-blue-700 rounded-lg">
            <span className="ml-2">🚧</span>
            <span className="font-medium">قريباً</span>
          </div>
        )}
      </div>
    </div>
  );
};

export default PagePlaceholder;