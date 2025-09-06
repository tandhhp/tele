import { apiCallStatistics } from "@/services/call";
import { ProCard, Statistic } from "@ant-design/pro-components"
import { useRequest } from "@umijs/max";

const CallStatistics: React.FC = () => {

    const { data } = useRequest(apiCallStatistics)

    return (
        <ProCard title="Thống kê cuộc gọi" headerBordered>
            <div>
                <Statistic value={data?.totalCurrentYear || 0}
                    layout="vertical"
                    title="Cuộc gọi trong năm" tip="Cuộc gọi trong năm" />
            </div>
            <div className="mb-2">
                <span className="mr-2">Tháng trước: <span className="text-red-500">{data?.totalPreviousMonth || 0}</span></span>
                <span>Tháng này: <span className="text-green-500">{data?.totalCurrentMonth || 0}</span></span>
            </div>
            <div className="border-t border-dashed pt-1">
                <span className="text-slate-600 mr-2">Tổng số liên hệ:</span>
                <span>{data?.totalContacts || 0}</span>
            </div>
        </ProCard>
    )
}

export default CallStatistics;