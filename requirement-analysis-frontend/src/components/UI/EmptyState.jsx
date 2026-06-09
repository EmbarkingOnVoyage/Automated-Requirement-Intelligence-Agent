export default function EmptyState({
  title,
  description,
  action,
}) {
  return (
    <div className="flex flex-col items-center justify-center py-20 text-center">

      <div className="text-6xl mb-4">
        📁
      </div>

      <h2 className="text-xl font-semibold text-slate-700">
        {title}
      </h2>

      <p className="text-slate-500 mt-2 max-w-md">
        {description}
      </p>

      {action && (
        <div className="mt-5">
          {action}
        </div>
      )}
    </div>
  );
}