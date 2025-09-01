import { apiContactStatistics } from "@/services/contact";
import { ProCard, Statistic } from "@ant-design/pro-components"
import { useRequest } from "@umijs/max";

const ContactStatistics: React.FC = () => {

    const { data } = useRequest(apiContactStatistics)

    return (
        <ProCard title="Thống kê liên hệ" headerBordered size="small">
            <div>
                <Statistic value={data?.totalCurrentYear || 0}
                    layout="vertical"
                    title="Liên hệ trong năm" tip="Liên hệ trong năm" />
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

export default ContactStatistics;