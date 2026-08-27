import Badge from '../components/UI/Badge';

function MOMDisplay({ content }) {
  // Try to parse as JSON first
  const parsed = parseMOMJson(content);

  if (parsed) {
    return (
      <div className="bg-blue-50 border border-blue-100 rounded-xl p-4 space-y-4 text-sm max-h-96 overflow-y-auto">

        {/* Header */}
        <div className="border-b border-blue-200 pb-3">
          <h4 className="font-bold text-slate-800 text-base">
            {parsed.meetingTitle || 'Minutes of Meeting'}
          </h4>
          {parsed.date && (
            <p className="text-slate-500 text-xs mt-1">📅 {parsed.date}</p>
          )}
        </div>

        {/* Attendees */}
        {parsed.attendees?.length > 0 && (
          <div>
            <p className="font-semibold text-slate-700 mb-2">👥 Attendees</p>
            <div className="flex flex-wrap gap-2">
              {parsed.attendees.map((a, i) => (
                <span key={i} className="bg-white border border-blue-200 px-2 py-1 rounded-lg text-xs text-slate-600">
                  {typeof a === 'object' ? `${a.name} (${a.role})` : a}
                </span>
              ))}
            </div>
          </div>
        )}

        {/* Agenda */}
        {parsed.agenda?.length > 0 && (
          <div>
            <p className="font-semibold text-slate-700 mb-2">📋 Agenda</p>
            <ul className="space-y-1">
              {parsed.agenda.map((a, i) => (
                <li key={i} className="text-slate-600 flex gap-2">
                  <span className="text-blue-400">{i + 1}.</span> {a}
                </li>
              ))}
            </ul>
          </div>
        )}

        {/* Discussion Points */}
        {parsed.discussionPoints?.length > 0 && (
          <div>
            <p className="font-semibold text-slate-700 mb-2">💬 Discussion Points</p>
            <div className="space-y-3">
              {parsed.discussionPoints.map((p, i) => (
                <div key={i} className="bg-white rounded-lg p-3 border border-blue-100">
                  <p className="font-medium text-slate-700">
                    {i + 1}. {typeof p === 'object' ? p.topic : p}
                  </p>
                  {p.discussion && (
                    <p className="text-slate-500 text-xs mt-1">
                      <span className="font-medium">Discussion:</span> {p.discussion}
                    </p>
                  )}
                  {p.decision && (
                    <p className="text-slate-600 text-xs mt-1">
                      <span className="font-medium text-green-600">✅ Decision:</span> {p.decision}
                    </p>
                  )}
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Action Items */}
        {parsed.actionItems?.length > 0 && (
          <div>
            <p className="font-semibold text-slate-700 mb-2">⚡ Action Items</p>
            <div className="space-y-2">
              {parsed.actionItems.map((a, i) => (
                <div key={i} className="bg-white rounded-lg p-3 border border-orange-100 flex gap-3">
                  <span className="text-orange-400 mt-0.5">→</span>
                  <div>
                    <p className="text-slate-700 font-medium text-xs">
                      {typeof a === 'object' ? a.action : a}
                    </p>
                    {a.owner && (
                      <p className="text-slate-400 text-xs mt-0.5">
                        Owner: <span className="text-blue-600">{a.owner}</span>
                        {a.dueDate && ` • Due: ${a.dueDate}`}
                      </p>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Open Issues */}
        {parsed.openIssues?.length > 0 && (
          <div>
            <p className="font-semibold text-slate-700 mb-2">⚠️ Open Issues</p>
            <ul className="space-y-1">
              {parsed.openIssues.map((issue, i) => (
                <li key={i} className="text-slate-600 text-xs flex gap-2">
                  <span className="text-red-400">•</span> {issue}
                </li>
              ))}
            </ul>
          </div>
        )}

        {/* Next Steps */}
        {parsed.nextSteps?.length > 0 && (
          <div>
            <p className="font-semibold text-slate-700 mb-2">🚀 Next Steps</p>
            <ul className="space-y-1">
              {parsed.nextSteps.map((step, i) => (
                <li key={i} className="text-slate-600 text-xs flex gap-2">
                  <span className="text-green-500">{i + 1}.</span> {step}
                </li>
              ))}
            </ul>
          </div>
        )}

        {/* Next Meeting */}
        {parsed.nextMeetingDate && (
          <div className="border-t border-blue-200 pt-3">
            <p className="text-slate-600 text-xs">
              📅 <span className="font-medium">Next Meeting:</span> {parsed.nextMeetingDate}
            </p>
          </div>
        )}
      </div>
    );
  }

  // Fallback — plain text
  return (
    <div className="bg-blue-50 border border-blue-100 rounded-xl p-4 text-sm text-slate-700 whitespace-pre-wrap max-h-80 overflow-y-auto">
      {content}
    </div>
  );
}

// ── Parse MOM from JSON string or nested object ────────────────────────
function parseMOMJson(content) {
  try {
    // If already an object
    if (typeof content === 'object') {
      return content.minutesOfMeeting || content;
    }

    // Try JSON parse
    const parsed = JSON.parse(content);

    // Handle nested { minutesOfMeeting: {...} }
    return parsed.minutesOfMeeting || parsed;
  } catch {
    // Plain text — return null to use fallback
    return null;
  }
}


function Section({
  title,
  items,
  icon,
  color = 'blue',
}) {

  if (!items || items.length === 0) {
    return null;
  }

  return (
    <div className="mb-6">

      <div className="flex items-center gap-2 mb-3">

        <h2 className="text-lg font-semibold text-slate-700">
          {icon} {title}
        </h2>

        <Badge
          text={items.length}
          color={color}
        />

      </div>

      <div className="space-y-2">

        {items.map((item, index) => (
          <div
            key={index}
            className="bg-slate-50 border border-slate-200 rounded-xl p-3 text-sm text-slate-700"
          >
            {item}
          </div>
        ))}

      </div>

    </div>
  );
}

export default function AnalysisResult({ data }) {
  console.log('AnalysisResult data:', data);
  if (!data) return null;

  return (
    <div className="bg-white rounded-xl shadow-sm border border-slate-100 p-6 h-full overflow-y-auto max-h-[80vh]">

      {/* Header */}
      <div className="mb-5 pb-4 border-b border-slate-100">
        <h2 className="text-lg font-bold text-slate-800">
          {data.projectTitle || 'Analysis Result'}
        </h2>
        {data.projectObjective && (
          <p className="text-slate-500 text-sm mt-1">{data.projectObjective}</p>
        )}
      </div>

      {/* ── MOM Section ───────────────────────────────────
      {data.minutesOfMeeting && (
        <div className="mb-6">
          <div className="flex items-center justify-between mb-3">
            <h3 className="text-sm font-semibold text-slate-700 flex items-center gap-2">
              📝 Minutes of Meeting
            </h3>
            <button
              onClick={() => {
                navigator.clipboard.writeText(data.minutesOfMeeting);
                alert('MOM copied to clipboard!');
              }}
              className="text-xs text-blue-600 hover:underline"
            >
              Copy MOM
            </button>
          </div>
          <div className="bg-blue-50 border border-blue-100 rounded-xl p-4 text-sm text-slate-700 whitespace-pre-wrap max-h-80 overflow-y-auto">
            {data.minutesOfMeeting}
          </div>
        </div>
      )} */}
      {data.minutesOfMeeting && (
  <div className="mb-6">
    <div className="flex items-center justify-between mb-3">
      <h3 className="text-sm font-semibold text-slate-700 flex items-center gap-2">
        📝 Minutes of Meeting
      </h3>
      <button
        onClick={() => {
          navigator.clipboard.writeText(data.minutesOfMeeting);
          alert('MOM copied!');
        }}
        className="text-xs text-blue-600 hover:underline"
      >
        Copy MOM
      </button>
    </div>
    <MOMDisplay content={data.minutesOfMeeting} />
  </div>
)}

      {/* ── Requirements Sections ────────────────────────── */}
      <Section icon="⚡" title="Functional Requirements"     items={data.functionalRequirements}    color="blue"   />
      <Section icon="🔒" title="Non-Functional Requirements" items={data.nonFunctionalRequirements} color="gray"   />
      <Section icon="👤" title="User Stories"                items={data.userStories}               color="purple" />
      <Section icon="📋" title="Business Rules"              items={data.businessRules}             color="green"  />
      <Section icon="🧩" title="Modules"                     items={data.modules}                   color="blue"   />
      <Section icon="🔗" title="API Suggestions"             items={data.apiSuggestions}            color="purple" />
      <Section icon="🗄️" title="Database Entities"          items={data.databaseEntities}          color="orange" />
      <Section icon="👥" title="Roles"                       items={data.roles}                     color="gray"   />
      <Section icon="⚠️" title="Communication Gaps"          items={data.communicationGaps}         color="orange" />
      <Section icon="🚩" title="Risk Flags"                  items={data.riskFlags}                 color="red"    />
      <Section icon="❓" title="Open Questions"              items={data.openQuestions}             color="orange" />
      <Section icon="🎯" title="Must Have"                   items={data.prioritization?.mustHave}  color="blue"   />
      <Section icon="📅" title="Milestones"                  items={data.suggestedMilestones}       color="green"  />
    </div>
  );
}