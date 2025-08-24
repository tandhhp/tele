import { apiSaleRevenueReport } from "@/services/user"
import { ProCard } from "@ant-design/pro-components"
import { useRequest } from "@umijs/max"
import { Alert, Statistic } from "antd";

const SaleRevenue: React.FC = () => {

    const { data } = useRequest(apiSaleRevenueReport);

    return (
        <div className="mb-4">
            <div className="grid grid-cols-2 md:grid-cols-5 gap-4 mb-4">
                <ProCard>
                    <Statistic title="Tổng doanh thu" value={data?.total} suffix="đ" />
                </ProCard>
                <ProCard>
                    <Statistic title="Chờ GD duyệt" value={data?.pending} suffix="đ" />
                </ProCard>
                <ProCard>
                    <Statistic title="Chờ KT duyệt" value={data?.accountant} suffix="đ" />
                </ProCard>
                <ProCard>
                    <Statistic title="Doanh thu tháng" value={data?.month} suffix="đ" />
                </ProCard>
                <ProCard>
                    <Statistic title="Doanh thu năm" value={data?.year} suffix="đ" />
                </ProCard>
            </div>
            <Alert message="Doanh thu được tính từ các giao dịch đã được duyệt bởi giám đốc và kế toán." type="info" showIcon className="mb-4" />
        </div>
    )
}

export default SaleRevenue