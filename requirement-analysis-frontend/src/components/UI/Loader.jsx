export default function Loader({ text = 'Loading...' }) {
  return (
    <div className="flex flex-col items-center justify-center py-20">
      <div className="w-14 h-14 border-4 border-blue-200 border-t-blue-600 rounded-full animate-spin" />

      <p className="mt-4 text-sm text-slate-500">
        {text}
      </p>
    </div>
  );
}