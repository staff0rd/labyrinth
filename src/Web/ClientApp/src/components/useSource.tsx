import { useSelector } from '../store/useSelector';
export const useSource = (network: string) => {
  const sourceId = useSelector(state => state.account.source[network]);
  const sources = useSelector(state => state.account.sources);
  const sourceName = () => {
    if (sourceId && sources && sources.length)
      return sources.find(s => s.id === sourceId)!.name;
  };
  return { sourceId, sourceName: sourceName(), sources: (sources||[]).filter(p => p.network === network) };
};
