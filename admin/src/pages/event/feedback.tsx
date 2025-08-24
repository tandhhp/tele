import { apiLeadFeedback } from "@/services/user";
import { PageContainer, ProTable } from "@ant-design/pro-components"
import { Tag } from "antd";
import dayjs from "dayjs";

const FeedbackPage: React.FC = () => {
    return (
        <PageContainer>
            <ProTable
                scroll={{
                    x: true
                }}
                request={apiLeadFeedback}
                search={{
                    layout: 'vertical'
                }}
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30
                    },
                    {
                        title: 'Họ và tên',
                        dataIndex: 'name'
                    },
                    {
                        title: 'SDT',
                        dataIndex: 'phoneNumber'
                    },
                    {
                        title: 'Tuổi',
                        dataIndex: 'age',
                        search: false,
                    },
                    {
                        title: 'Nghề nghiệp',
                        dataIndex: 'jobTitle',
                        search: false,
                    },
                    {
                        title: 'Yêu thích',
                        dataIndex: 'interestLevel',
                        search: false
                    },
                    {
                        title: 'Người TO',
                        dataIndex: 'to',
                        search: false
                    },
                    {
                        title: 'Lý do từ chối',
                        dataIndex: 'rejectReason',
                        search: false
                    },
                    {
                        title: 'Tài chính',
                        dataIndex: 'financialSituation',
                        search: false
                    },
                    {
                        title: 'Nguồn',
                        dataIndex: 'source',
                        search: false
                    },
                    {
                        title: 'Ngày sự kiện',
                        dataIndex: 'eventDate',
                        valueType: 'date',
                        search: false,
                        render: (_, entity) => entity.eventDate ? dayjs(entity.eventDate).format('DD-MM-YYYY') : '-',
                        width: 100
                    },
                    {
                        title: 'Giờ',
                        dataIndex: 'eventTime',
                        search: false
                    },
                    {
                        title: 'GTHĐ',
                        dataIndex: 'contractAmount',
                        search: false,
                        valueType: 'digit'
                    },
                    {
                        title: 'Đã TT',
                        dataIndex: 'amountPaid',
                        search: false,
                        valueType: 'digit'
                    },
                    {
                        title: 'Tầng',
                        dataIndex: 'floor',
                        search: false
                    },
                    {
                        title: 'Bàn',
                        dataIndex: 'tableName',
                        search: false
                    },
                    {
                        title: 'Checkin',
                        dataIndex: 'checkinTime',
                        search: false
                    },
                    {
                        title: 'Checkout',
                        dataIndex: 'checkoutTime',
                        search: false
                    },
                    {
                        title: 'Xét nghiệm tại chỗ',
                        dataIndex: 'isOnsiteTesting',
                        search: false,
                        valueEnum: {
                            false: <Tag>Không</Tag>,
                            true: <Tag>Có</Tag>
                        }
                    }
                ]}
            />
        </PageContainer>
    )
}

export default FeedbackPage;